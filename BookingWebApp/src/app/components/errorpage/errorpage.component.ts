import { Component } from '@angular/core';

import { TranslateService } from '@ngx-translate/core';
import { IconDefinition, faCircleExclamation, faBug} from '@fortawesome/free-solid-svg-icons';

import { BookingWebAPIErrorResponse } from 'src/shared/models';
import { ErrorService } from 'src/app/services';

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
