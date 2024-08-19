import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { ResourceCategoryViewModel } from 'src/app/modules/data-access/';
import { ResourceService } from 'src/app/modules/data-access/services';
import { DataGridColumnDefinition, DataGridComponent, DataType, EditAccess, FetchPageModel, DataValidation } from '../datagrid/datagrid.component';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  public welcomeMessage?: string;
  public gridData: DontForgetDeletingMe[] = [
    { username: "Peter", age: 41 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 40 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 39 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 48 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 371 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 461 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 451 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 441 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 431 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 421 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 411 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 401 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 491 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 4331 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 421 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 401 }, { username: "Barb", age: 34 }, { username: "Justin", age: 17 },
    { username: "Peter", age: 4221 }, { username: "Barb", age: 34 }, { username: "Justin99", age: 917 },
  ];
  public gridColumns: DataGridColumnDefinition[] = [ 
    { headerName: "DATAGRIDTEST.USERNAME", fieldName: "username", type: DataType.String, canEdit: false, validationRules: { notEmpty: null } },
    { headerName: "DATAGRIDTEST.AGE", fieldName: "age", type: DataType.Number, canEdit: false, validationRules: { numberMax: 50 } }
  ];

  constructor(private resourceService: ResourceService,  private translateService: TranslateService) {}
  
  public ngOnInit(): void {
    this.welcomeMessage = 'Welcome to the dark side!';
  }

  // TODO: this funtion is just for temporary purposes. Delete if not needed anymore.
  public fetchUsers: (pageSize: number, whichPage: number) => FetchPageModel<DontForgetDeletingMe> = (pageSize: number, whichPage: number) => {
    // 'whichPage' index starts from 1, not 0.
    let startIndex: number = (whichPage - 1) * pageSize;
    return {
      pageIndex: whichPage,
      pageItems: this.gridData.slice(startIndex, startIndex + pageSize),
      totalItems: this.gridData.length
    };
  }

  public editUser: (user: DontForgetDeletingMe) => void = (user) => {
    console.log(`Action modifying user info to: ${user.username}, ${user.age} has been fired.`);
  }
}

interface DontForgetDeletingMe {
  username: string;
  age: number;
}
