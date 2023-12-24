import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { Observable, Subject, catchError, map, of, tap } from 'rxjs';
import { BookingWebAPIAuthenticationViewModel, BookingWebAPIUserViewModel, LoginViewModel, UsersService } from 'src/app/modules/data-access';
import { BookingWebAPIErrorResponse } from 'src/shared/models/bookingWebAPIErrorResponse';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnDestroy {
  private loggedInUser: BookingWebAPIUserViewModel | null;
  public loginEvent$: Subject<BookingWebAPIUserViewModel | null>;

  constructor(private userService: UsersService) {
    this.loggedInUser = null;
    this.loginEvent$ = new Subject<BookingWebAPIUserViewModel | null>();
  }

  ngOnDestroy(): void {
    this.loginEvent$.complete();
  }

  public getLoggedInUser: () => Observable<BookingWebAPIUserViewModel | null> = () => {
    return this.loggedInUser !== null && this.loggedInUser !== undefined ?
      of(this.loggedInUser) :
      this.fetchLoggedInUser().pipe(
        map((success: boolean) => success ? this.loggedInUser : null)
      );
  };

  public tryLogin: (loginModel: LoginViewModel) => Observable<{success: boolean; user: BookingWebAPIUserViewModel | null; error: BookingWebAPIErrorResponse | null}> = (loginModel) => {
    return this.userService.apiUsersAuthenticatePost(loginModel).pipe(
      tap((authResponse: BookingWebAPIAuthenticationViewModel) => {
        // TODO: save the logged in user information to the store instead of passing it...
        this.loginEvent$.next(authResponse.user ?? null);
        this.loggedInUser = authResponse.user ?? null;
      }),
      map((authResponse: BookingWebAPIAuthenticationViewModel) => ({success: authResponse.token ? true : false, user: authResponse.user ?? null, error: null})),
      catchError((error: HttpErrorResponse) => of({success: false, user: null, error: error.error}))
    );
  };

  // Logs out the user from the application and deletes the cached user info from the service.
  public logout: () => Observable<boolean> = () => {
    return this.userService.apiUsersLogoutPost().pipe(
      tap((_: any) => {
        this.loginEvent$?.next(null);
        this.loggedInUser = null;
      }),
      map((_: any) => true),
      catchError((_: HttpErrorResponse) => of(false))
    );
  };

  // Fetches then caches logged in user info and returns true if the user info could be fetched - false otherwise.
  private fetchLoggedInUser: () => Observable<boolean> = () => {
    return this.userService.apiUsersLoggedInUserGet().pipe(
      tap((user: BookingWebAPIUserViewModel) => { this.loggedInUser = user; }),
      map((user: BookingWebAPIUserViewModel) => user !== null && user !== undefined),
      // Handling the case when the user is not logged in and we got a response with HTTP 401 (Unauthorized).
      catchError((_: HttpErrorResponse) => of(false))
    );
  }
}
