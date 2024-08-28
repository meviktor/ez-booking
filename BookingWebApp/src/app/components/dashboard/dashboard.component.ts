import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ResourceCategoryViewModel } from '../../model';
import { DataGridColumnDefinition, DataGridComponent, DataType, FetchPageModel } from '../datagrid/datagrid.component';
import { Store } from '@ngrx/store';
import { BookingWebAppState } from 'src/app/state/bookingwebappstate';
import { ResourceCategoryActionNames } from 'src/app/state/actions/resourcecategory.actions';
import { selectEntityListFromState } from 'src/app/state/selectors/generic.selectors';
import { map, Subscription } from 'rxjs';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  public welcomeMessage?: string = 'Welcome to the dark side!';
  public gridColumns: DataGridColumnDefinition[] = [ 
    { headerName: "CORE.NAME", fieldName: "name", type: DataType.String, canEdit: false, validationRules: { notEmpty: null } },
    { headerName: "CORE.DESCRIPTION", fieldName: "description", type: DataType.String, canEdit: false, validationRules: { notEmpty: null } }
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
              .subscribe(fpm => { this.gridData = fpm; });
  };

  public editUser: (user: any) => void = (user) => {
    console.log(`Action modifying user info to: ${user.username}, ${user.age} has been fired.`);
  }
}