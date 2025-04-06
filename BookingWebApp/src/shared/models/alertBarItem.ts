export type NotificationType = "info" | "success" | "warning" | "danger";

export interface AlertBarItem {
    type: NotificationType;
    message: string;
}
