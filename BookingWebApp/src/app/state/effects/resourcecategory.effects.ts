import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap } from 'rxjs/operators';
import { ResourceService } from '../../services';
import { getResourceCategories, getResourceCategoriesFailed, getResourceCategoriesSuccess, ResourceCategoryActionNames } from '../actions/resourcecategory.actions';

@Injectable()
export class ResourceCategoryEffects {

  loadResourceCategories$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(getResourceCategories),
        exhaustMap(() => this.resourceService.getAllResourceCategories()
          .pipe(
            map(rcs => ({ type: ResourceCategoryActionNames.getResourceCategoriesSuccess, resourceCategories: rcs })),
            catchError(error => of({ type: ResourceCategoryActionNames.getResourceCategoriesFailed, error: error }))
          )
        )
    )}
  );

  loadResourceCategoriesSuccess$ = createEffect(() => {
    return this.actions$.pipe(
        ofType( getResourceCategoriesSuccess ),
        tap(() => { console.log("Fetching resource categories has succeeded.") })
    )},
    { dispatch: false }
  );

  loadResourceCategoriesFailed$ = createEffect(() => {
    return  this.actions$.pipe(
        ofType(getResourceCategoriesFailed),
        tap(() => { console.log("Fetching resource categories has failed.") })
    )},
    { dispatch: false }
  );

  constructor(
    private actions$: Actions,
    private resourceService: ResourceService
  ) {}
}