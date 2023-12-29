import { inject } from "@angular/core";
import { AuthService } from "../services/authentication/auth.service";
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from "@angular/router";
import { BookingWebAPIUserViewModel } from "../../data-access";
import { map } from "rxjs";

const confirmUserGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    let authService: AuthService = inject(AuthService);
    let router: Router = inject(Router);
    return authService.getLoggedInUser().pipe(
        // if the user is logged in, it is not allowed to see the user accont confirmation page
        map((user: BookingWebAPIUserViewModel | null) => (user === null ? true: router.parseUrl('')))
    );
}

export default confirmUserGuard;
