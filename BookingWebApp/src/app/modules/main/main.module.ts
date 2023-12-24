import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MainRoutingModule } from './main-routing.module';
import { MainComponent } from './components/main/main.component';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { CookieHttpInterceptor } from './interceptors/cookieinterceptor';

@NgModule({
  declarations: [
    MainComponent,
    LoginComponent,
    DashboardComponent
  ],
  imports: [
    BrowserModule,
    MainRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [CookieService,  { provide: HTTP_INTERCEPTORS, useClass: CookieHttpInterceptor, multi: true }],
  bootstrap: [MainComponent]
})
export class MainModule { }
