import { FormConfig, FormField } from '@appTypes/form';

export const selectForm = (forms: Record<string, FormConfig>, formId: string): FormConfig => forms[formId] ? JSON.parse(JSON.stringify(forms[formId])) : null;
export const selectFormDefaultFields = (forms: Record<string, FormConfig>, formId: string): Array<FormField> => selectForm(forms, formId)?.steps?.[0]?.fields ?? [];
export const selectFormInitialValues = (forms: Record<string, FormConfig>, formId: string): Record<string, any> => selectForm(forms, formId)?.initialValues ?? {};
export const selectFormErrors = (forms: Record<string, FormConfig>, formId: string): Record<string, string> => selectForm(forms, formId)?.errors ?? {};
