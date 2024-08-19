import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, NgForm, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { catchError, of, take } from 'rxjs';
import { BookingWebAPIUserViewModel, UsersService } from 'src/app/modules/data-access/';
import { BookingWebAPIErrorResponse } from 'src/shared/models/bookingWebAPIErrorResponse';
import { arrayOfRange } from 'src/shared/utils';
import { IconDefinition, faPencil, faEye, faTrashCan, faSquareCheck, faSquareXmark} from '@fortawesome/free-solid-svg-icons'
import { v4 as uidGen } from 'uuid';

/**
 * Custom grid control for displaying items regardless of their type. Allows view, edit and delete actions via externally provided callbacks (via attributes). 
 */
@Component({
  selector: 'data-grid',
  templateUrl: './datagrid.component.html',
  styleUrls: ['./datagrid.component.css']
})
class DataGridComponent<TElement extends object> implements OnInit {
  /** Contains the definitions of the columns in the grid. */
  @Input() columnSettings!: DataGridColumnDefinition[];
  /** Controls if the view icon is available in the grid and so the view details functionality.
   *  If its value is not null, the view icon will be displayed for the grid items.
   */
  @Input() viewURL?: string = "<placeholder>";
  /** Controls if the edit icon is available in the grid and so the edit functionality. */
  @Input() editAllowed: boolean = true;
  /** Controls if the delete icon is available in the grid and so the delete functionality. */
  @Input() deleteAllowed: boolean = true;
  // TODO: they will potentially return promises... delete nulls.
  /**
   * Callback for fetching data for the grid control. 
   * The task of the implementer is to handle edge cases e.g. the tremendous amount of deleted item items between two fetches,
   * which could cause that the value of whichPage argument becomes too large (no such valid page index exists).
   @param pageSize number of items on a page.
   @param whichPage the index of the page which will be displayed in the grid (starts from 1).
   */
  @Input() fetchAction: (pageSize: number, whichPage: number) => FetchPageModel<TElement> = null!;
  /**
   * Callback for editing a grid item.
   @param editedItem the edited grid item.
   */
  @Input() editAction: (editedItem: TElement) => void = null!;
   /**
   * Callback for deleting a grid item.
   @param deletedItem the item selected for deletion.
   */
  @Input() deleteAction: (deletedItem: TElement) => void = null!;

  public DataValidationErrorCode = DataValidationErrorCode;
  /** Form used for inline editing. It's value is undefined if no item is being edited. */
  protected editForm!: FormGroup;

  // TODO: remove it in the long run?
  protected data?: DataGridInternalViewModel<TElement>[];

  protected faPencil: IconDefinition = faPencil;
  protected faEye: IconDefinition = faEye;
  protected faTrashCan: IconDefinition = faTrashCan;
  protected faCheck: IconDefinition = faSquareCheck;
  protected faXmark: IconDefinition = faSquareXmark;

  protected actualPage: number = 1;
  protected actualPageRange: number[] = [1];
  protected availablePageSizes: number[] = [5, 10, 25, 50];
  protected pageSize: number = this.availablePageSizes[0];
  protected itemsTotal: number = 0;
  protected pagesTotal: number = 1;
  protected Direction = Direction;
  protected rowUnderEdit?: string;

  ngOnInit(): void {
    this.loadPage(1);
  }

  protected onEditSubmit(){
    if(this.rowUnderEdit !== undefined && this.editForm !== undefined && this.editForm.valid){
      this.editAction(this.editForm.value);
    }
  }

  protected onEditCancel(): void{
    this.rowUnderEdit = undefined;
  }

  protected getInputType(dataType: DataType): string {
      switch(dataType){
        case DataType.String:
          return "text";
        case DataType.Number:
          return "number";
        case DataType.Boolean:
          return "checkbox";
        case DataType.Date:
          return "date";
        case DataType.DateTime:
          return "datetime-local";
      }
      // This line can run only if someone has added a new memeber to the DataType enum, but forgot to set its associated HTML input type in this method...
      throw Error(`Data type "${dataType}" does not have any HTML input type configured.`);
  }

  protected extractFieldValue(rowData: TElement, fieldName: string): any | undefined {
    if(!fieldName){
      return undefined;
    }

    let rowDataFieldNames: string[] = Object.keys(rowData);
    let rowDataValues: any[] = Object.values(rowData);
    let fieldIndex: number = rowDataFieldNames.indexOf(fieldName);

    return fieldIndex == -1 ? undefined : rowDataValues[fieldIndex];
  }

  protected oddClass(rowIndex: number) : string {
    return rowIndex % 2 == 1 ? 'odd' : '';
  }

  protected navOne(direction: Direction) : void {
    if((this.actualPage <= 1 && direction == Direction.Backward) || (this.actualPage >= this.pagesTotal && direction == Direction.Forward)) {
      return;
    }
    this.loadPage(this.actualPage + direction);
  }

  protected navTo(pageIndex: number) : void {
    if(pageIndex < 1 || pageIndex > this.pagesTotal || pageIndex == this.actualPage) {
      return;
    }
    this.loadPage(pageIndex);
  }

  protected pageSizeChanged(): void {
    this.loadPage(1);
  }

  protected loadPage(pageIndex: number): void {
    this.updateState(this.fetchAction(this.pageSize, pageIndex));
  }

  protected onEditClick(rowId: string): void {
    this.editForm = this.createFormGroup(rowId);
    this.rowUnderEdit = rowId;
  }

  protected editItem(editedItem: TElement): void {
    this.editAction(editedItem);
    // TODO: this has some issues, for example if you are on the last page and a lot of elements were deleted in the meantime, you can "run out" of pages...
    //this.loadPage(this.actualPage);
  }

  protected deleteItem(editedItem: TElement): void {
    this.deleteAction(editedItem);
    // TODO: this has some issues, for example if you are on the last page and a lot of elements were deleted in the meantime, you can "run out" of pages...
    //this.loadPage(this.actualPage);
  }

  protected createFormGroup(gridElementId: string): FormGroup {
    let { gridRowId, ...dataElement } = this.data!.find(e => e.gridRowId == gridElementId)!;
    let group: any = {};

    Object.entries(dataElement).map(keyValueArray => ({ key: keyValueArray[0], value: keyValueArray[1]})).forEach(kvp => {
      let columnDefinition = this.columnSettings.find(cs => cs.fieldName === kvp.key);
      if(columnDefinition !== undefined && columnDefinition !== null){
        group[kvp.key] = columnDefinition.validationRules ? 
          new FormControl(kvp.value, DataValidator.applyRules(columnDefinition.validationRules)) :
          new FormControl(kvp.value)
      }
      else {
        throw Error(`Field "${kvp.key}" of type "${typeof(dataElement)}" does not have any column setting defined.`);
      }
    });
    return new FormGroup(group);
  }

  private updateState(newData: FetchPageModel<TElement>): void {
    this.data = newData.pageItems.map(item => { return <DataGridInternalViewModel<TElement>>{...item, gridRowId: uidGen()} });
    this.itemsTotal = newData.totalItems;
    this.actualPage = newData.pageIndex;
    this.pagesTotal = Math.ceil(this.itemsTotal / this.pageSize);
    this.actualPageRange = this.calculatePageRange();
  }

  private calculatePageRange(): number[] {
    let lowerPageIndexes: number[] = arrayOfRange(1, this.actualPage);
    let higherPageIndexes: number[] = arrayOfRange(this.actualPage + 1, this.pagesTotal + 1);
    // Always returning (maximum) 5 page indexes regardless what is the index of the page (lowest: 1) actually showing in tge grid.
    if(lowerPageIndexes.length < 2){
      return lowerPageIndexes.length == 0 ? [this.actualPage, ...higherPageIndexes.slice(0, 4)] : [lowerPageIndexes[0], this.actualPage, ...higherPageIndexes.slice(0, 3)]
    }
    if(higherPageIndexes.length < 2){
      return higherPageIndexes.length == 0 ? [...lowerPageIndexes.slice(-4), this.actualPage] : [...lowerPageIndexes.slice(-3), this.actualPage, higherPageIndexes[0]]
    }
    return [...lowerPageIndexes.slice(-2), this.actualPage, ...higherPageIndexes.slice(0, 2)]
  }
}

/**
 * Internal data grid view model, which adds some technical fields to the generic type.
 */
type DataGridInternalViewModel<T> = T & { 
  /**
   * Unique identifier of a data grid row.
   */
  gridRowId: string
};

/**
 * Interface exposing options of the grid columns.
 */
interface DataGridColumnDefinition {
  /**
   * Translation text identifier for the column header.
   */
  headerName: string;
  /**
   * Name of the field which will be displayed in the column.
   */
  fieldName: string;
  /**
   * Type of the field which will be displayed in the column.
   */
  type: DataType;
  /**
   * Specifies if the column supports editing.
   */
  canEdit: boolean;
  /**
   * Specifies the width of the column if specified. The whole grid has 12 width units (comes from bootstrap).
   */
  widthProportion?: number;
  /**
   * Contains the list of validation rules applied to the column in case of inline editing.
   * Keys has to be chosen from the {@link DataValidation} type. The keys also refer to the type of the value has to be chosen for the validation rule.
   * E.g. when the rule StringMinLength is chosen, then the value must have the string type.
   */
  validationRules?: { [key: string]: string | number | Date | null }
}

/**
 * Interface containing relevant information of a data fetch.
 */
interface FetchPageModel<T>{
  /**
   * Items on the fetched page/fraction of the full data set.
   */
  pageItems: T[];
  /**
   * Index of the fetched page (starts from 1).
   */
  pageIndex: number;
  /**
   * Number of total items could be fetched from the backend.
   */
  totalItems: number;
}

enum Direction { Forward = 1, Backward = -1 };

enum DataType { String, Number, Boolean, Date, DateTime } 

enum EditAccess { ReadOnly, ReadWrite } 

const DataValiationTypes =  ["notEmpty" , "stringMinLength" , "stringMaxLength" , "numberMin" , "numberMax" , "numberInteger" , "dateFrom" , "dateTo"]
/**
 * String literal type containing the possible validation rules.
 * Possible values: notEmpty, stringMinLength, stringMaxLength, numberMin, numberMax, numberInteger, dateFrom, dateTo.
 */
type DataValidation = typeof DataValiationTypes[number]

/**
 * Contains the minimal validation logic provided by the grid control for inline editing.
 * For more complicated validation rules, use a stand-alone Angular component (page) with a form instead of inline editing.
 */
class DataValidator
{
  /**
   * Checks a set of rules on a value provided in a form field.
   * @param rules The set of rules which will be checked.
   * @returns A {@link ValidatorFn} checks wheter the value in the {@link AbstractControl} (input) complies the rules.
   */
  public static applyRules: (rules: { [key: string]: string | number | Date | null }) => ValidatorFn = (rules) => {
    let invalidRules: string[] = Object.keys(rules).filter(rule => !DataValiationTypes.includes(rule));
    if(invalidRules.length != 0){
      throw new Error(`Unknown validation rules: ${invalidRules.join(", ")}.`);
    }

    return (control: AbstractControl): ValidationErrors | null => {
      let validationErrors: any = [];
      Object.keys(rules).forEach(ruleName => {
        let errorCode: DataValidationErrorCode | null = this.checkRule(ruleName, control.value, rules[ruleName]);
        if(errorCode != null){
          validationErrors.push(errorCode);
        }
      });
      return validationErrors.length == 0 ? null : { validationErrorCodes: validationErrors }
    };
  };

  /**
   * Performs a single rule check.
   * @param rule The rule to check.
   * @param value The examined value.
   * @param limit The limit tied to the rule.
   * @returns An error code if the rule is not fulfilled, {@link null} otherwise.
   */
  private static checkRule: (rule: string, value?: string | number | Date, limit?: string | number | Date | null) => DataValidationErrorCode | null = (rule, value, limit) => {
    switch(rule){
      case "notEmpty":
        return this.notEmpty(value);
      case "stringMinLength":
        if(typeof(value) !== 'string' || typeof(limit) !== 'number'){
          break;
        }
        return this.string_minLength(limit, value);
      case "stringMaxLength":
        if(typeof(value) !== 'string' || typeof(limit) !== 'number'){
          break;
        }
        return this.string_maxLength(limit, value);
      case "numberMin": 
        if((typeof(value) !== 'number' && Number(value) === Number.NaN) || (typeof(limit) !== 'number' && Number(limit) === Number.NaN)){
          break;
        }
        return this.number_minValue(Number(limit), Number(value));
      case "numberMax":
        if((typeof(value) !== 'number' && Number(value) === Number.NaN) || (typeof(limit) !== 'number' && Number(limit) === Number.NaN)){
          break;
        }
        return this.number_maxValue(Number(limit), Number(value));
      case "numberInteger":
        if((typeof(value) !== 'number' && Number(value) === Number.NaN)){
          break;
        }
        return this.number_integer(Number(value));
      case "dateFrom":
        // TODO: handle incoming date in string format!
        if(!(value instanceof Date) || !(limit instanceof Date)){
          break;
        }
        return this.date_fromValue(limit, value);
      case "dateTo": 
       // TODO: handle incoming date in string format!
        if(!(value instanceof Date) || !(limit instanceof Date)){
          break;
        }
        return this.date_toValue(limit, value);
    }
    throw new Error(`Ivalid limit (${limit}) or value (${value}) for rule '${rule}'.`);
  };

  private static notEmpty: (input?: string | number | Date) => DataValidationErrorCode | null = (input) => (input !== null && input !== undefined && (typeof(input) == 'string' ? /\S/.test(input) : true)) ? null : DataValidationErrorCode.NotEmpty;

  private static string_minLength: (length: number, input?: string) => DataValidationErrorCode | null = (length, input) => (input !== null && input !== undefined && input.length >= length) ? null : DataValidationErrorCode.StringTooShort;

  private static string_maxLength: (length: number, input?: string) => DataValidationErrorCode | null = (length, input) => (input == null || input == undefined || input.length <= length) ? null : DataValidationErrorCode.StringTooLong;

  private static number_minValue: (min: number, input?: number) => DataValidationErrorCode | null = (min, input) => (input !== null && input !== undefined && input >= min) ? null : DataValidationErrorCode.NumberTooLow;

  private static number_maxValue: (max: number, input?: number) => DataValidationErrorCode | null = (max, input) => (input == null || input == undefined || input <= max) ? null : DataValidationErrorCode.NumberTooHigh;

  private static number_integer: (input?: number) => DataValidationErrorCode | null = (input) => (input == null || input == undefined || input % 1 != 0) ? null : DataValidationErrorCode.NumberNotInteger;

  private static date_fromValue: (min: Date, input?: Date) => DataValidationErrorCode | null = (min, input) => (input !== null && input !== undefined && input >= min) ? null : DataValidationErrorCode.DateTooLow;

  private static date_toValue: (max: Date, input?: Date) => DataValidationErrorCode | null = (max, input) => (input == null || input == undefined || input <= max) ? null : DataValidationErrorCode.DateTooHigh;
}

enum DataValidationErrorCode {
  NotEmpty = 'NotEmpty',
  StringTooShort = 'StringTooShort',
  StringTooLong = 'StringTooLong',
  NumberTooLow = 'NumberTooLow',
  NumberTooHigh = 'NumberTooHigh',
  NumberNotInteger = 'NumberNotInteger',
  DateTooLow = 'DateTooLow',
  DateTooHigh = 'DateTooHigh'
}

export { DataGridComponent, DataGridColumnDefinition, DataType, EditAccess, FetchPageModel, DataValidation }
