type snapCallback = (str: string) => void;

interface IWebcam {
	attach: (string) => void;
	freeze: () => void;
	set: (any) => void;
	snap: (snapCallback) => void;
	unfreeze: () => void;
}

declare var Webcam: IWebcam;

