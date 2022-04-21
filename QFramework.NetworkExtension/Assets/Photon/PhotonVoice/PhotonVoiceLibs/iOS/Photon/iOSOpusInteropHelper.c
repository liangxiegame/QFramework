int opus_encoder_ctl(void *st, int request, ...);
int opus_decoder_ctl(void *st, int request, ...);

int opus_encoder_ctl_set_ext(void *st, int request, int value) {
    return opus_encoder_ctl(st, request, value);
}
int opus_encoder_ctl_get_ext(void *st, int request, int* value) {
    return opus_encoder_ctl(st, request, value);
}
int opus_decoder_ctl_set_ext(void *st, int request, int value) {
    return opus_decoder_ctl(st, request, value);
}
int opus_decoder_ctl_get_ext(void *st, int request, int* value) {
    return opus_decoder_ctl(st, request, value);
}
