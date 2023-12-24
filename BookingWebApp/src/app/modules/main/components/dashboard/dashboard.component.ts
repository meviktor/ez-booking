import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ResourceCategoryViewModel } from 'src/app/modules/data-access/';
import { ResourceService } from 'src/app/modules/data-access/services';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private resourceCategorySubscription?: Subscription;
  
  public welcomeMessage?: string;
  public resourceCategory?: ResourceCategoryViewModel;

  constructor(private resourceService: ResourceService) {}

  public ngOnDestroy(): void {
    this.resourceCategorySubscription?.unsubscribe();
  }
  
  public ngOnInit(): void {
    this.welcomeMessage = 'Welcome to the dark side!';
    // TODO: this is just for demonstration purposes! Trying out to access a protected api endpoint after a successful login!
    this.resourceCategorySubscription = this.resourceService.getResourceCategoryById('E9AEA1A5-702E-4B80-90D5-D0F00DB2FF9B').subscribe(this.setResourceCategory);
  }

  private setResourceCategory: (rc: ResourceCategoryViewModel) => void = (rc) => this.resourceCategory = rc;
}
