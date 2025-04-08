import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

import { TranslateService } from '@ngx-translate/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap } from 'rxjs/operators';

import * as ACTIONS from 'src/app/state/actions';
import { ResourceCategoryActionNames } from 'src/app/state/actions';
import { AlertService } from 'src/app/services';
import { ResourceService } from 'src/app/services';
import { NotificationType } from 'src/shared/models';

@Injectable()
export class ResourceCategoryEffects {

  private displayNotification(messageId: string, notificationType: NotificationType) : void
  {
    this.translateService.get(messageId).subscribe(translatedMessage => this.alertService.sendAlert({type: notificationType, message: translatedMessage}));
  }

  loadResourceCategories$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.getResourceCategories),
        exhaustMap(() => this.resourceService.getAllResourceCategories()
          .pipe(
            map(rcs => ({ type: ResourceCategoryActionNames.getResourceCategoriesSuccess, resourceCategories: rcs })),
            catchError((error: HttpErrorResponse) => of({ type: ResourceCategoryActionNames.getResourceCategoriesFailed, error }))
          )
        )
    )}
  );

  loadResourceCategoriesSuccess$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.getResourceCategoriesSuccess)
    )},
    { dispatch: false }
  );

  loadResourceCategoriesFailed$ = createEffect(() => {
    return  this.actions$.pipe(
        ofType(ACTIONS.getResourceCategoriesFailed),
        tap(() => this.displayNotification("RESOURCECATEGORY.CREATEORUPDATESUCCESS", "danger"))
    )},
    { dispatch: false }
  );

  createOrUpdateResourceCategory$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.createOrUpdateResourceCategory),
        exhaustMap((action) => this.resourceService.createOrUpdateResourceCategory(action.resourceCategory)
          .pipe(
            map(rc => ({ type: ResourceCategoryActionNames.createOrUpdateResourceCategorySuccess, resourceCategory: rc })),
            catchError((error: HttpErrorResponse) => of({ type: ResourceCategoryActionNames.createOrUpdateResourceCategoryFailed, error }))
          )
        )
    )}
  );

  createOrUpdateResourceCategorySuccess$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.createOrUpdateResourceCategorySuccess),
        tap(() => this.displayNotification("RESOURCECATEGORY.CREATEORUPDATESUCCESS", "success"))
    )},
    { dispatch: false }
  );

  createOrUpdateResourceCategoryFailed$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.createOrUpdateResourceCategoryFailed),
        tap(() => this.displayNotification("RESOURCECATEGORY.CREATEORUPDATEFAILED", "danger"))
    )},
    { dispatch: false }
  );

  deleteResourceCategory$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.deleteResourceCategory),
        exhaustMap((action) => this.resourceService.deleteResourceCategory(action.resourceCategory)
          .pipe(
            map(id => ({ type: ResourceCategoryActionNames.deleteResourceCategorySuccess, id: id })),
            catchError((error: HttpErrorResponse) => of({ type: ResourceCategoryActionNames.deleteResourceCategoryFailed, error }))
          )
        )
    )}
  );

  deleteResourceCategorySuccess$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.deleteResourceCategorySuccess),
        tap(() => this.displayNotification("RESOURCECATEGORY.DELETESUCCESS", "success"))
    )},
    { dispatch: false }
  );

  deleteResourceCategoryFailed$ = createEffect(() => {
    return this.actions$.pipe(
        ofType(ACTIONS.deleteResourceCategoryFailed),
        tap(() => this.displayNotification("RESOURCECATEGORY.DELETEFAILED", "danger"))
    )},
    { dispatch: false }
  );

  constructor(
    private actions$: Actions,
    private resourceService: ResourceService,
    private alertService: AlertService,
    private translateService: TranslateService
  ) {}
}
