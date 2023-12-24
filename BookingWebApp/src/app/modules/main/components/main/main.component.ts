import { Component, OnDestroy } from '@angular/core';
import { BookingWebAPIUserViewModel } from 'src/app/modules/data-access';
import { AuthService } from '../../services/authentication/auth.service';
import { Router } from '@angular/router';
import { Subscription, take } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnDestroy {
  private updateUserSubscription: Subscription;

  public appBrandName: string = "ez-booking";
  public loggedInUser: BookingWebAPIUserViewModel | null;

  constructor(private authService: AuthService, private router: Router) {
    this.loggedInUser = null;

    // fetching if we have a user with living session when the page is opened / reloaded
    this.authService.getLoggedInUser().pipe(take(1)).subscribe(this.setMenuVisibility);
    // handling login/logout events
    this.updateUserSubscription = this.authService.loginEvent$?.subscribe(this.setMenuVisibility);
  }

  ngOnDestroy(): void {
    this.updateUserSubscription.unsubscribe();
  }

  public setMenuVisibility: (loggedInUser: BookingWebAPIUserViewModel | null) => void = (loggedInUser) => this.loggedInUser = loggedInUser;

  public logout: () => void = () => {
    this.authService.logout().pipe(take(1)).subscribe(success => {
      if(success) {
        this.router.navigate(['/login']);
      }
      else {
        // TODO: the return value may will be changed to return an error object to be able to provide an adequate error message if an error occurred during logout.
        console.log("An error occurred during the user's logout attempt.");
      }
    });
  };
}
