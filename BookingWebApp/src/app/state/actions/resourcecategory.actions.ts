import { createAction, emptyProps, props } from '@ngrx/store';
import { ResourceCategoryViewModel } from 'src/app/model';

export const ResourceCategoryActionNames = {
  getResourceCategories: "getResourceCategories",
  getResourceCategoriesSuccess: "getResourceCategoriesSuccess",
  getResourceCategoriesFailed: "getResourceCategoriesFailed",
  createOrUpdateResourceCategory: "createOrUpdateResourceCategory",
  createOrUpdateResourceCategorySuccess: "createOrUpdateResourceCategorySuccess",
  createOrUpdateResourceCategoryFailed: "createOrUpdateResourceCategoryFailed",
  deleteResourceCategory: "deleteResourceCategory",
  deleteResourceCategorySuccess: "deleteResourceCategorySuccess",
  deleteResourceCategoryFailed: "deleteResourceCategoryFailed"
};

export const getResourceCategories = createAction(ResourceCategoryActionNames.getResourceCategories, props<{ resourceCategories: ResourceCategoryViewModel[] }>());
export const getResourceCategoriesSuccess = createAction(ResourceCategoryActionNames.getResourceCategoriesSuccess, props<{ resourceCategories: ResourceCategoryViewModel[] }>());
export const getResourceCategoriesFailed = createAction(ResourceCategoryActionNames.getResourceCategoriesFailed, emptyProps);

export const createOrUpdateResourceCategory = createAction(ResourceCategoryActionNames.createOrUpdateResourceCategory, props<{ resourceCategory: ResourceCategoryViewModel }>());
export const createOrUpdateResourceCategorySuccess = createAction(ResourceCategoryActionNames.createOrUpdateResourceCategorySuccess, props<{ resourceCategory: ResourceCategoryViewModel }>());
export const createOrUpdateResourceCategoryFailed = createAction(ResourceCategoryActionNames.createOrUpdateResourceCategoryFailed, emptyProps);

export const deleteResourceCategory = createAction(ResourceCategoryActionNames.deleteResourceCategory, props<{ resourceCategory: ResourceCategoryViewModel }>());
export const deleteResourceCategorySuccess = createAction(ResourceCategoryActionNames.deleteResourceCategorySuccess, props<{ id: string }>());
export const deleteResourceCategoryFailed = createAction(ResourceCategoryActionNames.deleteResourceCategoryFailed, emptyProps);
