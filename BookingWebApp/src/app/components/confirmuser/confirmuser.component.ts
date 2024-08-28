import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { catchError, of, take } from 'rxjs';
import { BookingWebAPIUserViewModel } from '../../model';
import { UsersService } from '../../api/api';
import { BookingWebAPIErrorResponse } from 'src/shared/models/bookingWebAPIErrorResponse';


@Component({
  selector: 'app-confirmuser',
  templateUrl: './confirmuser.component.html',
  styleUrls: ['./confirmuser.component.css']
})
export class ConfirmUserComponent {
  public confirmUserForm: FormGroup<ConfirmUserFormControls>;
  public confirmUserFormSubmitted: boolean = false;
  public confirmUserFinished: boolean = false;
  // TODO: implement 48 hours expiration!
  public userTokenExpired: boolean = false;
  public userToConfirm?: BookingWebAPIUserViewModel;
  public passwordPolicy?: { [key: string]: any };

  private userToken: string | null;

  constructor(private formBuilder: FormBuilder, private userService: UsersService,  private translateService: TranslateService, private router: Router, private route: ActivatedRoute) {
    this.confirmUserForm = this.buildConfirmUserForm();

    this.userToken = this.route.snapshot.paramMap.get('userToken');
    if(this.userToken !== null){
      // TODO: take token from URL
      //this.userService.apiUsersConfirmUserGet(this.userToken).pipe(take(1)).subscribe(this.fillConfirmUserData);
    }
    // TODO: else redirect to HTTP 404 page
    else console.error("No user token was provided in the URL.");
  }

  private buildConfirmUserForm = (): FormGroup<ConfirmUserFormControls> => this.formBuilder.group<ConfirmUserFormControls>({
    id: new FormControl<string | null>(null, Validators.required),
    email: new FormControl<string | null>(null, [Validators.required, Validators.email]),
    password: new FormControl<string | null>(null, Validators.required),
    confirmPassword: new FormControl<string | null>(null, [Validators.required, this.confirmPasswordMatch]), // TODO: custom validator function: match with passowrd
  });

  private fillConfirmUserData: (confirmUserData: any) => void = (confirmUserData) => {
    this.userToConfirm = confirmUserData.user;
    this.confirmUserForm.setValue({
      id: confirmUserData.user?.id,
      email: confirmUserData.user?.email,
      password: null,
      confirmPassword: null 
    });
    
    //this.passwordPolicy = {};
    //confirmUserData.passwordSettings?.forEach(ps => this.passwordPolicy![ps.name!] = ps);
  };

  private confirmPasswordMatch: (confirmPasswordControl: AbstractControl) => ValidationErrors | null = (confirmPasswordControl) => {
    var passwordValue = (confirmPasswordControl.parent as FormGroup<ConfirmUserFormControls>)?.controls.password.value;
    return passwordValue === confirmPasswordControl.value ?
      null :
      { passwordConfirmPasswordDoNotMatch: true};
  };

  public submitUserConfirmation: () => void = () => {
    this.confirmUserFormSubmitted = true;
    // clearing root level (not bound to any of the controls) errors before submit attempt
    this.confirmUserForm.setErrors(null);

    if(this.confirmUserForm.valid){
      // this.userService.apiUsersConfirmUserPost({ userId: this.confirmUserForm.controls.id.value!, token: this.userToken!, password: this.confirmUserForm.controls.password.value! })
      // .pipe(
      //   take(1),
      //   catchError((error: HttpErrorResponse) => of<BookingWebAPIErrorResponse>(error.error))
      // )
      // .subscribe(confirmResponse => {
      //     let confirmedUser = confirmResponse as BookingWebAPIUserViewModel; 
      //     if(confirmedUser.emailConfirmed === true) {
      //       this.confirmUserFinished = true;
      //     }
      //     else if (confirmedUser.emailConfirmed === false) {
      //       this.confirmUserForm.setErrors({errorAfterSubmit: 'CONFIRMUSERCOMPONENT.ERRORS.CONFIRMUSERFAILURE'});
      //     }
      //     else {
      //       // setting the error from the backend on root level
      //       this.confirmUserForm.setErrors({errorAfterSubmit: (confirmResponse as BookingWebAPIErrorResponse).errorCode});
      //     }
      //   }
      // );
    }
  };
}

interface ConfirmUserFormControls {
  id: FormControl<string | null | undefined>;
  email: FormControl<string | null | undefined>;
  password: FormControl<string | null | undefined>;
  confirmPassword: FormControl<string | null | undefined>;
}
