import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import authGuard from './guards/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import confirmUserGuard from './guards/confirmuser.guard';
import { EmailAddressConfirmationComponent } from './components/emailaddressconfirmation/emailaddressconfirmation.component';

const routes: Routes = [
  { path: '', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'emailaddressconfirmation/:confirmationAttemptId', component: EmailAddressConfirmationComponent, canActivate: [confirmUserGuard] },
  { path: 'login', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class MainRoutingModule { }
