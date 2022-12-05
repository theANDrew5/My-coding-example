export class DeliveryUtils {
    static isPointInBounds(bounds: number[][], pointCords: number[]) {
        var leftLat = bounds[0][0];
        var leftLng = bounds[0][1];
        var rightLat = bounds[1][0];
        var rightLng = bounds[1][1];

        var pointLat = pointCords[0];
        var pointLng = pointCords[1];

        return leftLat < pointLat && rightLat > pointLat && leftLng < pointLng && rightLng > pointLng;
    }
}