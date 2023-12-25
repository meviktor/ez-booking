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

export { setLocale }
