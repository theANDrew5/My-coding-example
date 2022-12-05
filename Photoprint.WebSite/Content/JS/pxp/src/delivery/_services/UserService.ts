import { pxpGlobal } from '../../globals/pxp';
import { DeliveryDisplayType } from '../controllers/DeliveryTypesSelector';

export class UserService {
    static getUserData(): Promise<IUserServiceGetUserDataResponse> {
        return new Promise((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open("GET", `/api/delivery/userData?frontendId=${pxpGlobal.frontend.frontendId}`);
            xhr.setRequestHeader("Content-Type", "application/json;charset=utf-8");
            xhr.onreadystatechange = () => {
                if (xhr.readyState !== XMLHttpRequest.DONE) return;
                if (xhr.status > 199 && xhr.status < 300) {
                    const response: IUserServiceGetUserDataResponse = JSON.parse(xhr.responseText);
                    resolve(response);
                }
                else {
                    reject(xhr.responseText);
                }
            };
            xhr.send();
        });
    }
}

export interface IUserServiceGetUserDataResponse {
    LastAddress: string;
    LastShippingId: number;
    LastShippingType: DeliveryDisplayType;
    CanBeOrderByUserCompany: boolean;
    Recipient: {
        Comment: null | string;
        Email: string;
        FirstName: string;
        LastName: string;
        MiddleName: null | string;
        Phone: string;
    };
}