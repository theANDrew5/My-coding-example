import template from './html/VDPSettings.html'
import { LocalizableString, ILocalizableString, LocalizableTextBox } from '../../controls/common/LocalizableTextBox';
import { TextBox } from '../../controls/common/TextBox';
import { rs } from '../../globals/pxp';

class VdpField {
	templateNameTextBox: TextBox;
	templateName: KnockoutObservable<string>;
	titleTextBox: LocalizableTextBox;
	title: KnockoutObservable<ILocalizableString>;
	constructor(state: IVDPField | null) {
		this.templateNameTextBox = new TextBox(state?.TemplateName, { undoEnable: false });
		this.templateName = this.templateNameTextBox.value;

		this.titleTextBox = new LocalizableTextBox(state?.Title ?? new LocalizableString(), { undoEnable: false })
		this.title = this.titleTextBox.value;
	}

	toJSON() {
		var copy = ko.toJS(this);
		delete copy.templateNameTextBox;
		delete copy.titleTextBox;
		return copy;
	};
}

interface IVDPField {
	TemplateName: string;
	Title: ILocalizableString;
}

export interface IVDPSettings {
	Enabled: boolean;
	Fields: Array<IVDPField>;
	GroupTitle: ILocalizableString;
}
export interface IVDPSettingsLocalization {
	VDPTitleText: string;
	VDPAddButtonText: string;
	VDPDeleteButtonText: string;
	VDPFieldsLabel: string;
	VDPTemplateTitleLabel: string;
	VDPTemplateLayerLabel: string;
}

export class VDPSettings {
	groupTitleTextBox: LocalizableTextBox;
	groupTitle: KnockoutObservable<ILocalizableString>;
	enabled: KnockoutObservable<boolean>;
	fields: KnockoutObservableArray<VdpField>;
	localization: IVDPSettingsLocalization;
	constructor(container: HTMLDivElement, state: IVDPSettings | null) {
		this.groupTitleTextBox = new LocalizableTextBox(state?.GroupTitle ?? new LocalizableString(), { label: rs.Print.Template.Info.VDPGroupLabel, undoEnable: false })
		this.groupTitle = this.groupTitleTextBox.value;
		this.enabled = ko.observable(state != null && state.Enabled == true);
		this.fields = ko.observableArray();
		this.localization = 
		{
			VDPTitleText: rs.Print.Template.Info.VDPSettingsTitle,
			VDPAddButtonText: rs.Print.Template.Info.VDPAddButton,
			VDPDeleteButtonText: rs.Print.Template.Info.VDPDeleteButton,
			VDPFieldsLabel: rs.Print.Template.Info.VDPFieldsLabel,
			VDPTemplateTitleLabel: rs.Print.Template.Info.VDPTemplateTitleLabel,
			VDPTemplateLayerLabel: rs.Print.Template.Info.VDPTemplateLayerLabel

		};
		if (state != null) {
			for (let i = 0; i < state.Fields.length; i++) {
				this.fields.push(new VdpField(state.Fields[i]));
			}
		}

		this.removeField = this.removeField.bind(this);
		this.addField = this.addField.bind(this);


		container.innerHTML = "";
		container.appendChild(document.createRange().createContextualFragment(template));
		ko.applyBindings(this, container);
	}

	removeField(field: VdpField) {
		this.fields.remove(field);
	}
	addField() {
		this.fields.push(new VdpField(null));
	}
}
