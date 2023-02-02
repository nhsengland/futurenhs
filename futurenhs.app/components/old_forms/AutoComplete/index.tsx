import { useEffect, useRef } from 'react'
import classNames from 'classnames'
import Autocomplete from 'accessible-autocomplete/react'

import { getSiteUsersByTerm } from '@services/getSiteUsersByTerm'
import { RichText } from '@components/generic/RichText'
import { RemainingCharacterCount } from '@components/old_forms/RemainingCharacterCount'
import { Option } from '@appTypes/option'
import { Service } from '@appTypes/service'

import { Props } from './interfaces'

/**
 * Derived from the NHS Design System Text Input component: https://service-manual.nhs.uk/design-system/components/text-input.
 * Used to allow users to enter text that’s no longer than a single line, such as their name or phone number.
 * Progressively enhanced with autocomplete functionality using the alphagov component: https://github.com/alphagov/accessible-autocomplete.
 * Use for autocomplete instances where the option count is in excess of several hundred - otherwise prefer to progressively enhance a select element.
 */
export const AutoComplete: (props: Props) => JSX.Element = ({
    input: { name, value, onChange },
    initialError,
    meta: { touched, error, submitError },
    text,
    shouldPreventFreeText,
    shouldRenderRemainingCharacterCount,
    minimumCharacters = 2,
    validators,
    context,
    services = {
        getSiteUsersByTerm,
    },
    serviceId,
    className,
}) => {
    const cache = useRef({})
    const serviceTimeOut = useRef(null)
    const autoCompleteField = useRef(null)
    const currentInputValue = useRef(null)

    const { label, hint } = text ?? {}

    const id: string = name
    const shouldRenderError: boolean =
        Boolean(initialError) ||
        ((Boolean(error) || Boolean(submitError)) && touched)
    const isRequired: boolean = Boolean(
        validators?.find(({ type }) => type === 'required')
    )
    const maxLength: boolean = validators?.find(
        ({ type }) => type === 'maxLength'
    )?.maxLength

    const generatedIds: any = {
        remainingCharacters: `${name}-remaining-characters`,
    }

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-form-group', className, {
            ['nhsuk-form-group--error']: shouldRenderError,
        }),
        label: classNames('nhsuk-label'),
        hint: classNames('nhsuk-hint'),
        error: classNames('nhsuk-error-message'),
        input: classNames('nhsuk-input nhsuk-u-width-full', {
            ['nhsuk-input--error']: shouldRenderError,
            ['u-clearfix']: shouldRenderRemainingCharacterCount && maxLength,
        }),
    }

    /**
     * Get suggestion list via an injected service using the input term
     */
    const handleGetSuggestions = async (
        term: string,
        callBack: Function
    ): Promise<void> => {
        const service: Service = services[serviceId]
        const cachedData: Array<Option> = cache.current?.[term]

        if (!service) {
            throw new Error('Invalid service')
        }

        /**
         * Return cached response to the input term if it exists
         */
        if (cachedData) {
            callBack(cachedData)
        } else {
            window.clearTimeout(serviceTimeOut.current)

            /**
             * Set a short timeout to prevent the service getting hit on each key press
             */
            serviceTimeOut.current = window.setTimeout(() => {
                /**
                 * Get suggestions from service
                 */
                service(Object.assign({ ...context, term }))
                    .then(({ data }) => {
                        /**
                         * Clear the suggestions cache if full
                         */
                        if (Object.keys(cache.current).length > 10) {
                            cache.current = {}
                        }

                        /**
                         * Add the suggestions to cache
                         */
                        cache.current[term] = data

                        /**
                         * Set the suggestions
                         */
                        callBack(data)
                    })
                    .catch((error) => {
                        console.log(error)
                    })
            }, 500)
        }
    }

    /**
     * Handle a confirmed selection
     */
    const handleSuggestionConfirm = (selection: Option): void => {
        if (selection) {
            onChange(selection.value)
            currentInputValue.current = selection.label
        }
    }

    /**
     * Handle a change to the autocomplete input element
     */
    const handleInputChange = (event: any): void => {
        const inputValue: string = event.target.value

        /**
         * If free text entry is prevented clear the underlying value
         */
        if (shouldPreventFreeText && inputValue !== currentInputValue.current) {
            const clearedValue: string = ''

            onChange(clearedValue)
            currentInputValue.current = clearedValue
        }
    }

    /**
     * Set up event listeners and timeouts
     */
    useEffect(() => {
        autoCompleteField.current = document.getElementById(name)
        autoCompleteField.current?.addEventListener('keyup', handleInputChange)
        autoCompleteField.current?.addEventListener('change', handleInputChange)

        return () => {
            window.clearTimeout(serviceTimeOut?.current)
        }
    }, [])

    /**
     * Render
     */
    return (
        <div className={generatedClasses.wrapper}>
            <label htmlFor={id} className={generatedClasses.label}>
                {label}
            </label>
            {hint && (
                <RichText
                    id={generatedIds.hintId}
                    className={generatedClasses.hint}
                    bodyHtml={hint}
                    wrapperElementType="span"
                />
            )}
            {shouldRenderError && (
                <span className={generatedClasses.error}>
                    {error || submitError || initialError}
                </span>
            )}
            <Autocomplete
                id={name}
                name={name}
                autoselect={true}
                confirmOnBlur={true}
                templates={{
                    inputValue: (value) => value?.label ?? '',
                    suggestion: ({ label }) => label,
                }}
                minLength={minimumCharacters}
                displayMenu="overlay"
                required={isRequired}
                showNoOptionsFound={false}
                defaultValue={value}
                source={handleGetSuggestions}
                onConfirm={handleSuggestionConfirm}
            />
            {shouldRenderRemainingCharacterCount && maxLength && (
                <RemainingCharacterCount
                    id={generatedIds.remainingCharacters}
                    currentCharacterCount={value?.length ?? 0}
                    maxCharacterCount={maxLength}
                    className="u-float-right"
                />
            )}
        </div>
    )
}
