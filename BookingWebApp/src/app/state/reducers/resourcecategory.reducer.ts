import { createReducer, on } from '@ngrx/store';

import { getResourceCategoriesSuccess, createOrUpdateResourceCategorySuccess, deleteResourceCategorySuccess } from '../actions/resourcecategory.actions';
import { ResourceCategoryViewModel } from '../../model';

export const initialStateResourceCategory: ReadonlyArray<ResourceCategoryViewModel> = [];

export const resourceCategoryReducer = createReducer(
  initialStateResourceCategory,
  on(getResourceCategoriesSuccess, (_state, { resourceCategories }) => resourceCategories),
  on(createOrUpdateResourceCategorySuccess, (_state, { resourceCategory }) => _state.filter(rc => rc.id != resourceCategory.id).concat(resourceCategory)),
  on(deleteResourceCategorySuccess, (_state, { id }) => _state.filter(rc => rc.id != id))
);