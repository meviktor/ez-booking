import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { BookingWebAPIErrorResponse } from 'src/shared/models/bookingWebAPIErrorResponse';
import { IconDefinition, faCircleExclamation, faBug} from '@fortawesome/free-solid-svg-icons';
import { ErrorService } from '../../services/error/error.service';

@Component({
  selector: 'app-errorpage',
  templateUrl: './errorpage.component.html',
  styleUrls: ['./errorpage.component.css']
})
export class ErrorPageComponent {
  public errorObject: BookingWebAPIErrorResponse | null;

  public faExclamation: IconDefinition = faCircleExclamation;
  public faBug: IconDefinition = faBug;

  constructor(private errorService: ErrorService,  private translateService: TranslateService) {
    this.errorObject = this.errorService.getLastError();
  }
}
