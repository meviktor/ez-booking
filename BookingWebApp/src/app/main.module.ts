// Angular
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule, HttpClient } from '@angular/common/http';
import { Configuration } from './configuration';
// Other npm dependencies
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NgbAlertModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
// Components
import { AlertBarComponent } from './components';
import { ConfirmUserComponent } from './components';
import { DashboardComponent } from './components';
import { DataGridComponent } from './components';
import { EmailAddressConfirmationComponent } from './components';
import { ErrorPageComponent } from './components';
import { LoginComponent } from './components';
import { LoginSlideShowComponent } from './components';
import { MainComponent } from './components';
// Pipes
import { DataGridVisibleColumnsPipe } from "./pipes";
// State
import { resourceCategoryReducer } from './state/reducers';
import { ResourceCategoryEffects } from './state/effects';
// Interceptors
import { CookieHttpInterceptor } from './interceptors';
// Modules
import { MainRoutingModule } from './main-routing.module';
import { environment } from 'src/environments/environment';
import { ApiModule } from './api.module';

// TODO: try out file loader instead of HTTP loader
function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, '../../../assets/i18n/', '.json');
}

/**
 * Injecting the right configuration with the correct backend API URL
 * for local development environment and production.
 * @returns The configuration used for injection.
 */
function APIConfigFactory(): Configuration {
  return new Configuration({
    basePath: environment.apiBaseUrl
  });
}

@NgModule({
  declarations: [
    MainComponent,
    LoginComponent,
    DashboardComponent,
    LoginSlideShowComponent,
    ConfirmUserComponent,
    EmailAddressConfirmationComponent,
    ErrorPageComponent,
    DataGridComponent,
    AlertBarComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MainRoutingModule,
    NgbModule,
    NgbAlertModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    FontAwesomeModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    StoreModule.forRoot({ resourceCategories: resourceCategoryReducer }),
    EffectsModule.forRoot([ResourceCategoryEffects]),
    ApiModule.forRoot(APIConfigFactory),
    
    // todo: what does this pipe doing here?
    DataGridVisibleColumnsPipe
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: CookieHttpInterceptor, multi: true }],
  bootstrap: [MainComponent]
})
export class MainModule { }
