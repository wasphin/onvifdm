
#pragma once

typedef void* onvifmp;
typedef void (*onvifmp_error_handler)(const char *aError);

enum LogType
{
  LOG_ERROR = 0,
  LOG_WARNING,
  LOG_INFORMATION,
};
typedef void (*onvifmp_log_handler)(const char *aMsg, const char *aSource,
                                    LogType aType);

//return null if failed
extern "C" __declspec(dllexport)
onvifmp onvifmp_initialize(onvifmp_error_handler aErrorHandler,
                           onvifmp_log_handler aLogHandler);
extern "C" __declspec(dllexport)
void onvifmp_close(onvifmp aInstance);

enum OnvifmpPixelFormat
{
  ONVIFMP_PF_RGB32 = 0
};

typedef void (*onvifmp_meta_callback)(const unsigned char* aBuffer,
                                      unsigned int aSize);

//return 0 if all ok
//aSilentMode = 1 - silent
//aSilentMode = 0 - noisy
extern "C" __declspec(dllexport)
int onvifmp_start_parsing(onvifmp aInstance, const char *aUrl,
                          int aWidth, int aHeight, int aStride, int pixFormat,
                          const char *aMapName,
                          onvifmp_meta_callback aCallback,
                          int aSilentMode);

//aSilentMode = 1 - silent
//aSilentMode = 0 - noisy
//return 0 if all ok
extern "C" __declspec(dllexport)
int onvifmp_player_set_silent_mode(onvifmp aInstance, const char *aUrl,
                                  int aSilentMode);

//return 0 if all ok
extern "C" __declspec(dllexport)
int onvifmp_stop_parsing(onvifmp aInstance, const char *aUrl);

extern "C" __declspec(dllexport)
int onvifmp_start_record(onvifmp aInstance, const char *aUrl,
                        const char *aFilePath);
extern "C" __declspec(dllexport)
int onvifmp_stop_record(onvifmp aInstance, const char *aUrl);