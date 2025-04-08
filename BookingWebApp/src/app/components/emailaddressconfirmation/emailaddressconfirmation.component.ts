import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { IconDefinition, faCircleCheck, faCircleXmark} from '@fortawesome/free-solid-svg-icons';
import { TranslateService } from '@ngx-translate/core';
import { catchError, of, take } from 'rxjs';

import { EmailConfirmationResultViewModel } from 'src/app/model';
import { UsersService } from 'src/app/api/api';
import { BookingWebAPIErrorResponse } from 'src/shared/models';

import { ErrorService } from 'src/app/services';


@Component({
  selector: 'app-emailaddressconfirmation',
  templateUrl: './emailaddressconfirmation.component.html',
  styleUrls: ['./emailaddressconfirmation.component.css']
})
export class EmailAddressConfirmationComponent {
  public confirmationSucceeded?: boolean;

  public faCircleCheck: IconDefinition = faCircleCheck;
  public faCircleXmark: IconDefinition = faCircleXmark;

  constructor(private userService: UsersService, private errorService: ErrorService, private translateService: TranslateService, private router: Router, private route: ActivatedRoute) {

    let attemptId = this.route.snapshot.paramMap.get('confirmationAttemptId');
    if(attemptId !== null){
      this.userService.apiUsersConfirmEmailAddressResultGet(attemptId!).pipe(
        take(1),
        catchError((error: HttpErrorResponse) => of<BookingWebAPIErrorResponse>(error.error))
      ).subscribe(this.setConfirmationSucceeded);
    }
    else {
      this.errorService.setLastError({errorCode: 'EntityNotFound', statusCode: 404});
      this.router.navigate(['error']);
    }
  }

  private setConfirmationSucceeded: (confirmationResultOrError: EmailConfirmationResultViewModel | BookingWebAPIErrorResponse) => void = (confirmationResultOrError) => {
    let result = confirmationResultOrError as EmailConfirmationResultViewModel;
    if(result.success !== undefined) {
      this.confirmationSucceeded = result.success;
    }
    else {
      let error = confirmationResultOrError as BookingWebAPIErrorResponse;
      this.errorService.setLastError(error);
      this.router.navigate(['error']);
    }
  }
}
