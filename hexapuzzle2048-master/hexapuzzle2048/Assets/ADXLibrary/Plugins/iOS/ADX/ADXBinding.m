//
//  ADXBinding.m
//  ADXLibrary_Example
//
//  Created by Eleanor Choi on 2018. 6. 15..
//  Copyright © 2018년 Chiung Choi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <ADXLibrary/ADXGDPR.h>

extern void UnityPause(int pause);
extern void UnitySendMessage(const char *, const char *, const char *);

static char* cStringCopy(NSString* input)
{
    const char* string = [input UTF8String];
    return string ? strdup(string) : NULL;
}

void _showADXConsent() {
    UnityPause(true);
    [ADXGDPR.sharedInstance showADXConsent:^(ADXConsentState consentState, BOOL success) {
        NSString *consentString = [[NSString alloc]initWithFormat:@"%li", (long)consentState];
        NSString *eventName = @"EmitADXConsentCompletion";
        
        NSData* data = [NSJSONSerialization dataWithJSONObject:@[consentString] options:0 error:nil];
        UnityPause(false);
        UnitySendMessage("ADXGDPR", eventName.UTF8String, [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding].UTF8String);
    }];
}

void _setDebugState(int state) {

// ADXDebugLocateDefault       = 0
// ADXDebugLocateInEEA         = 1
    
    [ADXGDPR.sharedInstance setDebugState:state];
}

int _getConsentState() {
    return [ADXGDPR.sharedInstance getConsentState];
}

void _setConsentState(int state) {
    
//    ADXConsentStateUnknown      = 0
//    ADXConsentStateNotRequired  = 1
//    ADXConsentStateDenied       = 2
//    ADXConsentStateConfirm      = 3
    
    [ADXGDPR.sharedInstance setConsentState:state];
}

const char*_getPrivacyPolicyURL() {
    return cStringCopy([ADXGDPR.sharedInstance getPrivacyPolicyURL].absoluteString);
}
