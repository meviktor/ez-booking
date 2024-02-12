import { Injectable } from '@angular/core';
import { BookingWebAPIErrorResponse } from 'src/shared/models/bookingWebAPIErrorResponse';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {
  private lastError: BookingWebAPIErrorResponse | null = null;

  public getLastError: () => BookingWebAPIErrorResponse | null = () => this.lastError;

  public setLastError: (error: BookingWebAPIErrorResponse) => void = (error) => this.lastError = error;
}
