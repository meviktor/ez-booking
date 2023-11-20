export * from './resourceCategory.service';
import { ResourceCategoryService } from './resourceCategory.service';
export * from './users.service';
import { UsersService } from './users.service';
export const APIS = [ResourceCategoryService, UsersService];
