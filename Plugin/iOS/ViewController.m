//
//  ViewController.m
//  iOSUnityIn
//
//  Created by 凉鞋 on 31/5/16.
//  Copyright © 2016年 葡萄科技. All rights reserved.
//

#import "ViewController.h"

@interface ViewController ()

@end

@implementation ViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

char *_receive(char * str1,char * str2)
{
    NSLog(@"%@,%@",[NSString stringWithUTF8String:str1],[NSString stringWithUTF8String:str2]);
    
    UnitySendMessage("Main Camera","unityGet",str2);
    
    return MakeStringCopy(str1);
}


// 防止内存泄露造成的闪退
char* MakeStringCopy(const char *string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res,string);
    return res;
}

@end
