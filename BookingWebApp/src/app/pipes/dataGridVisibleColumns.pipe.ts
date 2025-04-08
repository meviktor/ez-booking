import { Pipe, PipeTransform } from '@angular/core';

import { DataGridColumnDefinition } from '../components';

@Pipe({
  standalone: true,
  name: 'visibleColumns',
})
export class DataGridVisibleColumnsPipe implements PipeTransform {
  transform(value: DataGridColumnDefinition[]): DataGridColumnDefinition[] {
    return value.filter(v => v.visible === true);
  }
}
