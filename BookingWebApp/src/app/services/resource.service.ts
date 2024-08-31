import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, of, throwError } from 'rxjs';
import { ResourceCategoryService } from '../api/api';
import { ResourceCategoryViewModel } from '../model';

@Injectable({
  providedIn: 'root'
})
export class ResourceService {
  constructor(private resourceCategoryService: ResourceCategoryService) { }

  public getAllResourceCategories: () => Observable<ResourceCategoryViewModel[]> = () => {
    return this.resourceCategoryService.apiResourceCategoryGet();
  };

  public getResourceCategoryById: (id: string) => Observable<ResourceCategoryViewModel> = (id) => {
    return this.resourceCategoryService.apiResourceCategoryIdGet(id).pipe(
      catchError((error: HttpErrorResponse) => of(error.error))
    );
  };

  public createOrUpdateResourceCategory: (rc: ResourceCategoryViewModel) => Observable<ResourceCategoryViewModel> = (rc) => {
    return this.resourceCategoryService.apiResourceCategoryPut(rc);
  };

  public deleteResourceCategory: (rc: ResourceCategoryViewModel) => Observable<string> = (rc) => {
    return this.resourceCategoryService.apiResourceCategoryDelete(rc);
  };
}
