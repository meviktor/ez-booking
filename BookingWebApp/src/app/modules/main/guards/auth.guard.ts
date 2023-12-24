import { inject } from "@angular/core";
import { AuthService } from "../services/authentication/auth.service";
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from "@angular/router";
import { BookingWebAPIUserViewModel } from "../../data-access";
import { map } from "rxjs";

const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    let authService: AuthService = inject(AuthService);
    let router: Router = inject(Router);
    return authService.getLoggedInUser().pipe(
        map((user: BookingWebAPIUserViewModel | null) => (user !== null ? true : router.parseUrl('/login')))
    );
}

export default authGuard;