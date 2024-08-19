import { TranslateService } from '@ngx-translate/core';
import { LocaleCode } from './enums';

/**
 * Sets the currently used locale for the translation service used by the application. 
 * @param translateService the translation service instance used by the application.
 * @param localeCode the localization code which determines which language will be used.
 */
function setLocale(translateService: TranslateService, localeCode: LocaleCode){
    translateService.setDefaultLang(LocaleCode.English);
    translateService.use(localeCode);
}

/**
 * Returns an array containing the elements falling in the provided range.
 * @param start items in the array will start from this value.
 * @param end exclusive upper limit. This is the first item will not included by the returned array. 
 * @returns an array containing the numbers from start up to end - 1.
 */
function arrayOfRange(start: number, end: number): number[] {
    return Array.from({length: (end - start)}, (v, k) => k + start);
}

export { setLocale, arrayOfRange }
