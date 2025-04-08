import { createSelector } from '@ngrx/store';

import { BookingWebAppState } from 'src/app/state/bookingwebappstate';

/**
 * Selector for returning items from the state, optionally in paged/sorted form.
 * @param pageSize Optional. Number of items on a single page, if only a fraction of the whole state is required. Has no effect if pageIndex is not set.
 * @param pageIndex Optional. Index of the page (starting from 1), if only a fraction of the whole state is required to display. Has no effect if pageSize is not set.
 * @param orderBy Optional. Name of the field which defines the order of the items. Has no effect if asc is not set.
 * @param asc Optional. Determines the directon of sorting (true = ascending, false = descending). Has no effect if orderBy is not set.
 * @returns The desired set of items from the state determined by the above arguments.
 */
export function selectEntityListFromState<T>(stateFunc: (state: BookingWebAppState) => T[], pageSize?: number, pageIndex?: number, orderBy?: string, asc?: boolean) {
  return createSelector<BookingWebAppState, T[], { selectedItems: T[], totalItems: number }>(
    stateFunc,
    (items: T[]) => {
      let selectedItems: T[] = items;
      if (orderBy !== null && orderBy !== undefined) {
        if (asc !== null && asc !== undefined && items.length > 0) {
          selectedItems = selectedItems.sort((a: any, b: any) => asc ? 
            // ascending sorting
            (a[orderBy] < b[orderBy] ? -1 : (a[orderBy] > b[orderBy] ?  1 : 0)) :
            // descending sorting
            (a[orderBy] < b[orderBy] ?  1 : (a[orderBy] > b[orderBy] ? -1 : 0))
          )
        }
      }
      if (pageSize !== null && pageSize !== undefined && pageSize > 0 &&
          pageIndex !== null && pageIndex !== undefined && pageIndex > 0) {
          let startIndex = pageSize * (pageIndex - 1); // array index starts from 0, but pageIndex from 1
          // slice() returns an empty array in case if we are completely out of the array's bounds
          selectedItems = selectedItems.slice(startIndex, startIndex + pageSize);
      }
      return { selectedItems, totalItems: items.length };
    }
  );
};