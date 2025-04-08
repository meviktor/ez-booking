import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';

import { Store } from '@ngrx/store';
import { map, Subscription } from 'rxjs';

import { ResourceCategoryViewModel } from 'src/app/model';
import { BookingWebAppState } from 'src/app/state/';
import { ResourceCategoryActionNames } from 'src/app/state/actions/';
import { selectEntityListFromState } from 'src/app/state/selectors/';
import { DataGridColumnDefinition, DataGridComponent, DataType, FetchPageModel } from 'src/app/components';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  public welcomeMessage?: string = 'Welcome to the dark side!';
  public gridColumns: DataGridColumnDefinition[] = [ 
    { fieldName: "name", type: DataType.String, headerName: "CORE.NAME",  canEdit: false, validationRules: { notEmpty: null, stringMinLength: 10 }, visible: true },
    { fieldName: "description", type: DataType.String, headerName: "CORE.DESCRIPTION", canEdit: false, validationRules: { notEmpty: null }, visible: true },
    { fieldName: "id", visible: false },
  ];

  @ViewChild('mainDataGrid', { static: true }) dataGrid?: DataGridComponent<ResourceCategoryViewModel>;

  protected gridData?: FetchPageModel<ResourceCategoryViewModel>;
  protected gridDataSubscription?: Subscription;

  constructor(private store: Store<BookingWebAppState>) {}
  
  ngOnInit(): void {
    this.store.dispatch({ type: ResourceCategoryActionNames.getResourceCategories });
  }

  ngOnDestroy(): void {
    this.gridDataSubscription?.unsubscribe();
  }

  public fetchResourceCategories: (pageSize: number, whichPage: number) => void = (pageSize: number, whichPage: number) => {
    this.gridDataSubscription?.unsubscribe();
    this.gridDataSubscription = this.store.select(selectEntityListFromState(s => s.resourceCategories, pageSize, whichPage))
              .pipe(map(rcs => ({ totalItems: rcs.totalItems, pageIndex: whichPage, pageItems: rcs.selectedItems })))
              .subscribe(fpm => { this.gridData = fpm; console.log("New grid data arrived:"); console.log(fpm); });
  }

  public editResourceCategory: (resourceCategory: ResourceCategoryViewModel) => void = (resourceCategory) => {
    this.store.dispatch({ type: ResourceCategoryActionNames.createOrUpdateResourceCategory, resourceCategory });
  }

  public deleteResourceCategory: (resourceCategory: ResourceCategoryViewModel) => void = (resourceCategory) => {
    this.store.dispatch({ type: ResourceCategoryActionNames.deleteResourceCategory, resourceCategory });
  }
}
