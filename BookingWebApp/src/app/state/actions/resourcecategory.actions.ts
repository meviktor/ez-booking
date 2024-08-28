import { createAction, emptyProps, props } from '@ngrx/store';
import { ResourceCategoryViewModel } from '../../model';

export const ResourceCategoryActionNames = {
  getResourceCategories: "getResourceCategories",
  getResourceCategoriesSuccess: "getResourceCategoriesSuccess",
  getResourceCategoriesFailed: "getResourceCategoriesFailed",
  shouldNotBeSeen: "shouldNotBeSeen"
};

export const getResourceCategories = createAction(ResourceCategoryActionNames.getResourceCategories, props<{ resourceCategories: ResourceCategoryViewModel[] }>());
export const getResourceCategoriesSuccess = createAction(ResourceCategoryActionNames.getResourceCategoriesSuccess, props<{ resourceCategories: ResourceCategoryViewModel[] }>());
export const getResourceCategoriesFailed = createAction(ResourceCategoryActionNames.getResourceCategoriesFailed, emptyProps);
export const shouldNotBeSeen = createAction(ResourceCategoryActionNames.shouldNotBeSeen, emptyProps);
