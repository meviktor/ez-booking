import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { catchError, of, take } from 'rxjs';
import { EmailConfirmationResultViewModel, UsersService } from 'src/app/modules/data-access/';
import { BookingWebAPIErrorResponse } from 'src/shared/models/bookingWebAPIErrorResponse';
import { IconDefinition, faCircleCheck, faCircleXmark} from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'app-emailaddressconfirmation',
  templateUrl: './emailaddressconfirmation.component.html',
  styleUrls: ['./emailaddressconfirmation.component.css']
})
export class EmailAddressConfirmationComponent {
  public confirmationSucceeded?: boolean;

  public faCircleCheck: IconDefinition = faCircleCheck;
  public faCircleXmark: IconDefinition = faCircleXmark;

  constructor(private userService: UsersService,  private translateService: TranslateService, private router: Router, private route: ActivatedRoute) {

    let attemptId = this.route.snapshot.paramMap.get('confirmationAttemptId');
    if(attemptId !== null){
      this.userService.apiUsersConfirmEmailAddressResultGet(attemptId!).pipe(
        take(1),
        catchError((error: HttpErrorResponse) => of<BookingWebAPIErrorResponse>(error.error))
      ).subscribe(this.setConfirmationSucceeded);
    }
    // TODO: else redirect to HTTP 404 page
    // else console.error("No attempt id was provided in the URL.");
  }

  private setConfirmationSucceeded: (confirmationResultOrError: EmailConfirmationResultViewModel | BookingWebAPIErrorResponse) => void = (confirmationResultOrError) => {
    let result = confirmationResultOrError as EmailConfirmationResultViewModel;
    if(result.success !== undefined) {
      this.confirmationSucceeded = result.success;
    }
    // TODO: do something with the error message! Display it, or do a redirection somewhere else!
    else {
      let error = confirmationResultOrError as BookingWebAPIErrorResponse;
      console.log(error.errorCode)
    }
  }
}
