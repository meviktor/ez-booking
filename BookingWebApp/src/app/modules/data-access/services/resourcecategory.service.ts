import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { ResourceCategoryService, ResourceCategoryViewModel } from 'src/app/modules/data-access';

@Injectable({
  providedIn: 'root'
})
export class ResourceService {
  constructor(private resourceCategoryService: ResourceCategoryService) { }

  public getResourceCategoryById: (id: string) => Observable<ResourceCategoryViewModel> = (id) => {
    return this.resourceCategoryService.apiResourceCategoryIdGet(id).pipe(
      catchError((error: HttpErrorResponse) => of(error.error))
    );
  };
}
