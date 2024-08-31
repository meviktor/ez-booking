import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule, HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MainRoutingModule } from './main-routing.module';
import { MainComponent } from './components/main/main.component';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { CookieHttpInterceptor } from './interceptors/cookieinterceptor';
import { LoginSlideShowComponent } from './components/login/loginslideshow.component';
import { ConfirmUserComponent } from './components/confirmuser/confirmuser.component';
import { EmailAddressConfirmationComponent } from './components/emailaddressconfirmation/emailaddressconfirmation.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ErrorPageComponent } from './components/errorpage/errorpage.component';
import { DataGridComponent } from './components/datagrid/datagrid.component';
import { StoreModule } from '@ngrx/store';
import { resourceCategoryReducer } from './state/reducers/resourcecategory.reducer';
import { EffectsModule } from '@ngrx/effects';
import { ResourceCategoryEffects } from './state/effects/resourcecategory.effects';
import { DataGridVisibleColumnsPipe } from "./pipes/dataGridVisibleColumns.pipe";

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, '../../../assets/i18n/', '.json');
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
    DataGridComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MainRoutingModule,
    NgbModule,
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
    DataGridVisibleColumnsPipe
],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: CookieHttpInterceptor, multi: true }],
  bootstrap: [MainComponent]
})
export class MainModule { }
