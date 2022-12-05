export function getVDPSettings(container: HTMLDivElement, state: any) {
    return import(/* webpackChunkName: "VDPSettings" */'./VDPSettings')
        .then(VDPSettingsImport => {
            return new VDPSettingsImport.VDPSettings(container, state);
        })
        .catch((error) => {
            console.log(error);
        });
}