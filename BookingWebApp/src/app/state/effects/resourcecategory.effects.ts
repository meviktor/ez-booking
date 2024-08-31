import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap } from 'rxjs/operators';
import { ResourceService } from '../../services';
import * as ACTIONS from '../actions/resourcecategory.actions';
import { ResourceCategoryActionNames } from '../actions/resourcecategory.actions';

@Injectable()
export class ResourceCategoryEffects {

  loadResourceCategories$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.getResourceCategories),
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
        ofType(ACTIONS.getResourceCategoriesSuccess),
        tap(() => { console.log("Fetching resource categories has succeeded.") })
    )},
    { dispatch: false }
  );

  loadResourceCategoriesFailed$ = createEffect(() => {
    return  this.actions$.pipe(
        ofType(ACTIONS.getResourceCategoriesFailed),
        tap(() => { console.log("Fetching resource categories has failed.") })
    )},
    { dispatch: false }
  );

  createOrUpdateResourceCategory$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.createOrUpdateResourceCategory),
        exhaustMap((action) => this.resourceService.createOrUpdateResourceCategory(action.resourceCategory)
          .pipe(
            map(rc => ({ type: ResourceCategoryActionNames.createOrUpdateResourceCategorySuccess, resourceCategory: rc })),
            catchError(error => of({ type: ResourceCategoryActionNames.createOrUpdateResourceCategoryFailed, error: error }))
          )
        )
    )}
  );

  createOrUpdateResourceCategorySuccess$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.createOrUpdateResourceCategorySuccess),
        tap(() => { console.log("Creating/updating resource category has succeeded.") })
    )},
    { dispatch: false }
  );

  createOrUpdateResourceCategoryFailed$ = createEffect(() => {
    return  this.actions$.pipe(
        ofType(ACTIONS.createOrUpdateResourceCategoryFailed),
        tap(() => { console.log("Creating/updating resource category has failed.") })
    )},
    { dispatch: false }
  );

  deleteResourceCategory$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.deleteResourceCategory),
        exhaustMap((action) => this.resourceService.deleteResourceCategory(action.resourceCategory)
          .pipe(
            map(id => ({ type: ResourceCategoryActionNames.deleteResourceCategorySuccess, id: id })),
            catchError(error => of({ type: ResourceCategoryActionNames.deleteResourceCategoryFailed, error: error }))
          )
        )
    )}
  );

  deleteResourceCategorySuccess$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.deleteResourceCategorySuccess),
        tap(() => { console.log("Deleting resource category has succeeded.") })
    )},
    { dispatch: false }
  );

  cdeleteResourceCategoryFailed$ = createEffect(() => {
    return  this.actions$.pipe(
        ofType(ACTIONS.deleteResourceCategoryFailed),
        tap(() => { console.log("Deleting resource category has failed.") })
    )},
    { dispatch: false }
  );

  constructor(
    private actions$: Actions,
    private resourceService: ResourceService
  ) {}
}