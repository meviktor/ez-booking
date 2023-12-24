export * from './ping.service';
import { PingService } from './ping.service';
export * from './resourceCategory.service';
import { ResourceCategoryService } from './resourceCategory.service';
export * from './users.service';
import { UsersService } from './users.service';
export const APIS = [PingService, ResourceCategoryService, UsersService];
