import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { take } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

import { AuthService } from 'src/app/services';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public loginForm: FormGroup<LoginFormControls>;
  public loginFormSubmitted: boolean;
  
  constructor(private formBuilder: FormBuilder, private authService: AuthService, private translateService: TranslateService, private router: Router) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
    this.loginFormSubmitted = false;
  }

  public onLoginFormSubmit = (): void => {
    this.loginFormSubmitted = true;
    // clearing root level (not bound to any of the controls) errors before submit attempt
    this.loginForm.setErrors(null);

    if(this.loginForm.valid){
      this.authService.tryLogin({email: this.loginForm.value.email!, password: this.loginForm.value.password!}).pipe(take(1))
        .subscribe(authResult => {
          if(authResult.success){
            this.router.navigate(['/']);
          }
          else if(authResult.error){
            // setting the error from the backend on root level
            this.loginForm.setErrors({errorAfterSubmit: authResult.error.errorCode});
          }
        }
      );
    }
  };
}

interface LoginFormControls {
  email: FormControl<string | null>;
  password: FormControl<string | null>;
}
