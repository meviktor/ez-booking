import { createReducer, on } from '@ngrx/store';

import { getResourceCategoriesSuccess } from '../actions/resourcecategory.actions';
import { ResourceCategoryViewModel } from '../../model';

export const initialStateResourceCategory: ReadonlyArray<ResourceCategoryViewModel> = [];

export const resourceCategoryReducer = createReducer(
  initialStateResourceCategory,
  on(getResourceCategoriesSuccess, (_state, { resourceCategories }) => resourceCategories)
);