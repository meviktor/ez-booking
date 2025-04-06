import { AlertBarItem } from 'src/shared/models/alertBarItem';
import { Component } from '@angular/core';

@Component({
  selector: 'app-alertbar',
  templateUrl: './alertbar.component.html',
  styleUrls: ['./alertbar.component.css']
})
export class AlertBarComponent {
  protected alerts: AlertBarItem[] = [];

  constructor() { console.log(this.alerts); }

  public add(alert: AlertBarItem): void {
    this.alerts.push(alert);
  }

  public dismiss(alert: AlertBarItem): void {
		this.alerts.splice(this.alerts.indexOf(alert), 1);
	} 
}
