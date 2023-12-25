import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/authentication/auth.service';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public loginForm: FormGroup<LoginFormControls>;
  public loginFormSubmitted: boolean;
  
  constructor(private formBuilder: FormBuilder, private authService: AuthService, private translateService: TranslateService, private router: Router) {
    this.loginForm = this.buildLoginForm();
    this.loginFormSubmitted = false;
  }

  private buildLoginForm = (): FormGroup<LoginFormControls> => this.formBuilder.group<LoginFormControls>({
    email: new FormControl<string | null>('', [Validators.required, Validators.email]),
    password: new FormControl<string | null>('', Validators.required)
  });

  public onLoginFormSubmit = (): void => {
    this.loginFormSubmitted = true;
    if(this.loginForm.valid){
      this.authService.tryLogin({email: this.loginForm.value.email!, password: this.loginForm.value.password!}).pipe(take(1))
        .subscribe(authResult => {
            if(authResult.success){
              this.router.navigate(['/']);
            }
            else {
              // TODO: else inform about unsuccessful login attempt, using authResult.error
              console.log(authResult);
            }
          }
        );
    }
    console.log(this.loginForm.value);
  };
}

interface LoginFormControls {
  email: FormControl<string | null>;
  password: FormControl<string | null>;
}
