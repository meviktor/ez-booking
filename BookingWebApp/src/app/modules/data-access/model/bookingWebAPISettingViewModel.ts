/**
 * BookingWebAPI
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */
import { SettingCategory } from './settingCategory';
import { SettingValueType } from './settingValueType';


export interface BookingWebAPISettingViewModel { 
    category?: SettingCategory;
    name?: string | null;
    valueType?: SettingValueType;
    rawValue?: string | null;
}
export namespace BookingWebAPISettingViewModel {
}

