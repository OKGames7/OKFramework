#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/ATTrackingManager.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>

#ifdef __cplusplus
extern "C" {
#endif

// ATT 許可状態取得
// 0: Not Determined, 1: Restricted, 2: Denied, 3: Authorized, -1: No Needs
int GetTrackingAuthorizationStatus(){
    if (@available(iOS 14, *)) {
        return (int)ATTrackingManager.trackingAuthorizationStatus;
    } else {
        return -1;
    }
}

typedef void (*Callback)(int status);

// ATT 許可要求
// 0: Not Determined, 1: Restricted, 2: Denied, 3: Authorized, -1: No Needs
void RequestTrackingAuthorization(Callback callback)
{
    if (@available(iOS 14, *)) {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            if (callback != nil) {
                callback((int)status);
            }
        }];
    } else {
        callback(-1);
    }
}

#ifdef __cplusplus
}
#endif