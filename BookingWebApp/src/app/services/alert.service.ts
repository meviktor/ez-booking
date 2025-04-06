import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { AlertBarItem } from 'src/shared/models/alertBarItem';


@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private alerts: Subject<AlertBarItem> = new Subject<AlertBarItem>();

  public getAlertsSubject: () => Subject<AlertBarItem> = () => this.alerts;

  public sendAlert: (alert: AlertBarItem) => void = (alert) => this.alerts.next(alert);
}
