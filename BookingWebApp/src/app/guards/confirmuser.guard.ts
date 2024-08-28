import { inject } from "@angular/core";
import { AuthService } from "../services";
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from "@angular/router";
import { BookingWebAPIUserViewModel } from "../model";
import { map } from "rxjs";

const confirmUserGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    let authService: AuthService = inject(AuthService);
    let router: Router = inject(Router);
    return authService.getLoggedInUser().pipe(
        // if the user is logged in, it is not allowed to see the confirmation page
        map((user: BookingWebAPIUserViewModel | null) => (user === null ? true: router.parseUrl('')))
    );
}

export default confirmUserGuard;
