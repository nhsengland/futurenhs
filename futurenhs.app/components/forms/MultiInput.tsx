import { FormEvent, KeyboardEvent, useState } from 'react'
import { Input, Props as InputProps } from './Input'
import { mdiKeyboardReturn } from '@mdi/js'
import { SVGIcon } from '@components/generic/SVGIcon'

interface Props extends InputProps {
    getMulti?: (list: Array<string>) => void
}

export const MultiInput = ({ getMulti, ...inputProps }: Props) => {
    const { validation } = inputProps
    const [multi, setMulti] = useState<Array<string>>([])
    const [value, setValue] = useState<string>('')
    const [enterPress, setEnterPress] = useState<boolean>(false)

    const isError = validation && validation.some((v) => v.error === true)
    const hasOnKeyDown = typeof inputProps.onKeyDown === 'function'
    const hasOnKeyUp = typeof inputProps.onKeyDown === 'function'
    const hasOnChange = typeof inputProps.onChange === 'function'
    const hasGetMulti = typeof getMulti === 'function'

    const onKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
        const enterKey =
            (e.keyCode === 13 || e.key.toLowerCase() === 'enter') && !isError

        if (enterKey) {
            setEnterPress(true)
            handleMulti()
        }
    }

    const onKeyUp = (e: KeyboardEvent<HTMLInputElement>) => {
        const enterKey =
            (e.keyCode === 13 || e.key.toLowerCase() === 'enter') && !isError
        if (enterKey) {
            setEnterPress(false)
        }
    }

    const onChange = (e: FormEvent<HTMLInputElement>) => {
        const el = e.target as HTMLInputElement
        setValue(el.value)
    }

    const handleMulti = () => {
        const val = value
        const arr = multi
        setValue('')
        if (val && !arr.includes(val)) {
            const newArr = [...arr, val]
            setMulti(newArr)
            if (hasGetMulti) {
                getMulti(newArr)
            }
        }
    }

    return (
        <div style={{ position: 'relative' }}>
            <Input
                {...inputProps}
                value={value}
                onChange={(e) => {
                    onChange(e)
                    if (hasOnChange) inputProps.onChange(e)
                }}
                onKeyDown={(e) => {
                    onKeyDown(e)
                    if (hasOnKeyDown) inputProps.onKeyDown(e)
                }}
                onKeyUp={(e) => {
                    onKeyUp(e)
                    if (hasOnKeyUp) inputProps.onKeyUp(e)
                }}
            >
                <button
                    disabled={isError}
                    onClick={handleMulti}
                    className={`c-multi-input-enter-button ${
                        enterPress
                            ? 'c-multi-input-enter-button--pressed'
                            : null
                    } ${
                        isError ? 'c-multi-input-enter-button--disabled' : null
                    }`}
                >
                    <SVGIcon color="#fff" material name={mdiKeyboardReturn} />
                </button>
            </Input>
            {multi.map((text, i) => (
                <div
                    key={text + i}
                    className="c-button--alt u-mr-2 u-mb-2"
                    onClick={() =>
                        setMulti(multi.filter((item) => item !== text))
                    }
                >
                    {text} X
                </div>
            ))}
        </div>
    )
}
