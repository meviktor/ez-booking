import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import authGuard from './guards/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ConfirmUserComponent } from './components/confirmuser/confirmuser.component';
import confirmUserGuard from './guards/confirmuser.guard';

const routes: Routes = [
  { path: '', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'confirmuseraccount/:userToken', component: ConfirmUserComponent, canActivate: [confirmUserGuard] },
  { path: 'login', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class MainRoutingModule { }
