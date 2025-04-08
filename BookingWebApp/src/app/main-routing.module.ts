import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import * as Guards from 'src/app/guards';

import { DashboardComponent } from 'src/app/components';
import { EmailAddressConfirmationComponent } from 'src/app/components';
import { ErrorPageComponent } from 'src/app/components';
import { LoginComponent } from 'src/app/components';

const routes: Routes = [
  { path: '', component: DashboardComponent, canActivate: [Guards.authGuard] },
  { path: 'emailaddressconfirmation/:confirmationAttemptId', component: EmailAddressConfirmationComponent, canActivate: [Guards.confirmUserGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'error', component: ErrorPageComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class MainRoutingModule { }
