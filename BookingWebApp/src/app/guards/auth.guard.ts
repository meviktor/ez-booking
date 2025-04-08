import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from "@angular/router";

import { map } from "rxjs";

import { AuthService } from "src/app/services";
import { BookingWebAPIUserViewModel } from "src/app/model";

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    let authService: AuthService = inject(AuthService);
    let router: Router = inject(Router);
    return authService.getLoggedInUser().pipe(
        map((user: BookingWebAPIUserViewModel | null) => (user !== null ? true : router.parseUrl('/login')))
    );
}
