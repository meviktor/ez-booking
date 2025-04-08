import { createReducer, on } from '@ngrx/store';

import { getResourceCategoriesSuccess, createOrUpdateResourceCategorySuccess, deleteResourceCategorySuccess } from 'src/app/state/actions';
import { ResourceCategoryViewModel } from 'src/app/model';

export const initialStateResourceCategory: ReadonlyArray<ResourceCategoryViewModel> = [];

export const resourceCategoryReducer = createReducer(
  initialStateResourceCategory,
  on(getResourceCategoriesSuccess, (_state, { resourceCategories }) => resourceCategories),
  on(createOrUpdateResourceCategorySuccess, (_state, { resourceCategory }) => _state.filter(rc => rc.id != resourceCategory.id).concat(resourceCategory)),
  on(deleteResourceCategorySuccess, (_state, { id }) => _state.filter(rc => rc.id != id))
);